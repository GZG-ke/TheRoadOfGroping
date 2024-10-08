﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Common.Extensions;
using RoadOfGroping.Repository.Auditing;
using RoadOfGroping.Repository.Entities;
using RoadOfGroping.Repository.Entities.RoadOfGroping.Repository.Auditing;

namespace RoadOfGroping.Repository.UnitOfWorks
{
    public sealed class UnitOfWork<TDbContext> : IUnitOfWork, IDisposable, ITransientDependency where TDbContext : DbContext
    {
        public bool IsDisposed { get; private set; }

        public bool IsRollback { get; private set; }
        public bool IsCompleted { get; private set; }

        private readonly TDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAuditPropertySetter _auditPropertySetter;

        public UnitOfWork(TDbContext dbContext, IAuditPropertySetter auditPropertySetter)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException($"db context nameof{nameof(dbContext)} is null");
            _auditPropertySetter = auditPropertySetter;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            IsCompleted = false;
            return await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public IDbContextTransaction BeginTransaction()
        {
            IsCompleted = false;
            return _dbContext.Database.CurrentTransaction ?? _dbContext.Database.BeginTransaction();
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (IsCompleted)
            {
                return;
            }

            IsCompleted = true;
            ApplyChangeConventions();
            try
            {
                // 提交事务
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await _dbContext.Database.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // 发生异常回滚事务
                await RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
                throw new Exception(ex.Message);
            }
        }

        public void CommitTransaction()
        {
            if (IsCompleted)
            {
                return;
            }

            IsCompleted = true;
            ApplyChangeConventions();
            try
            {
                // 提交事务
                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();
            }
            catch (Exception x)
            {
                // 发生异常回滚事务
                RollbackTransaction();
                throw;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (IsCompleted)
            {
                return;
            }
            IsRollback = true;
            await _dbContext.Database.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public void RollbackTransaction()
        {
            if (IsCompleted)
            {
                return;
            }
            IsRollback = true;
            _dbContext.Database.RollbackTransaction();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyChangeConventions();
            return await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        private void ApplyChangeConventions()
        {
            _dbContext.ChangeTracker.DetectChanges();
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                PublishEventsForTrackedEntity(entry);
            }
        }

        private void PublishEventsForTrackedEntity(EntityEntry entry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyAbpConceptsForAddedEntity(entry);
                    break;

                case EntityState.Modified:
                    ApplyAbpConceptsForModifiedEntity(entry);
                    break;

                case EntityState.Deleted:
                    ApplyAbpConceptsForDeletedEntity(entry);
                    break;
            }
        }

        private void ApplyAbpConceptsForAddedEntity(EntityEntry entry)
        {
            CheckAndSetId(entry);
            SetCreationAuditProperties(entry);
        }

        private void ApplyAbpConceptsForDeletedEntity(EntityEntry entry)
        {
            if (!(entry.Entity is IDeletionAuditedEntity))
            {
                return;
            }

            entry.Reload();
            ObjectPropertyHelper.TrySetProperty(entry.Entity.As<IDeletionAuditedEntity>(), x => x.IsDeleted, () => true);
            SetDeletionAuditProperties(entry);
        }

        private void ApplyAbpConceptsForModifiedEntity(EntityEntry entry)
        {
            SetModificationAuditProperties(entry);
        }

        private void SetCreationAuditProperties(EntityEntry entry)
        {
            _auditPropertySetter?.SetCreationProperties(entry.Entity);
        }

        private void SetDeletionAuditProperties(EntityEntry entry)
        {
            _auditPropertySetter?.SetDeletionProperties(entry.Entity);
        }

        private void SetModificationAuditProperties(EntityEntry entry)
        {
            _auditPropertySetter?.SetModificationProperties(entry.Entity);
        }

        private void CheckAndSetId(EntityEntry entry)
        {
            if (entry.Entity is IEntity<Guid> entityWithGuidId)
            {
                TrySetGuidId(entry, entityWithGuidId);
            }

            if (entry.Entity is IEntity<string> entityWithStringId)
            {
                TrySetStringId(entry, entityWithStringId);
            }
        }

        private void TrySetGuidId(EntityEntry entry, IEntity<Guid> entity)
        {
            if (entity.Id != default)
            {
                return;
            }

            var idProperty = entry.Property("Id").Metadata.PropertyInfo;

            EntityHelper.TrySetId(
                entity,
                () => Guid.NewGuid(),
                true
            );
        }

        private void TrySetStringId(EntityEntry entry, IEntity<string> entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.Id) && entity.Id?.Length <= 32)
            {
                return;
            }

            var idProperty = entry.Property("Id").Metadata.PropertyInfo;

            EntityHelper.TrySetId(
                entity,
                () => Guid.NewGuid().ToString("N"),
                true
            );
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
        }
    }
}