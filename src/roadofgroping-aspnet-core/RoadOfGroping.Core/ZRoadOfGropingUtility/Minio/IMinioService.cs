﻿using Minio.DataModel;
using Minio.DataModel.Response;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Minio;

public interface IMinioService
{
    /// <summary>
    /// 创建存储桶
    /// </summary>
    /// <param name="bucketName"></param>
    /// <returns></returns>
    Task CreateBucketAsync(string bucketName);

    /// <summary>
    /// 删除存储桶
    /// </summary>
    /// <param name="bucketName"></param>
    /// <returns></returns>
    Task RemoveBucket(string bucketName);

    /// <summary>
    /// 获取存储桶存储对象数据流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<ObjectOutPut> GetObjectAsync(GetObjectInput input);

    /// <summary>
    /// 上传文件对象
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<PutObjectResponse> UploadObjectAsync(UploadObjectInput input);

    /// <summary>
    /// 上传本地文件使用默认存储桶
    /// </summary>
    /// <param name="uploads"></param>
    /// <returns></returns>
    Task UploadLocalUseDefaultBucket(List<UploadObjectInput> uploads);

    /// <summary>
    /// 创建默认存储桶
    /// </summary>
    /// <returns></returns>
    Task CreateDefaultBucket();

    /// <summary>
    /// 删除存储桶文件
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task RemoveObjectAsync(RemoveObjectInput input);

    /// <summary>
    /// 批量删除存储桶对象
    /// </summary>
    /// <param name="bucketName"></param>
    /// <param name="bucketNames"></param>
    /// <returns></returns>
    Task BatchRemoveObjectAsync(string bucketName, List<string> objectNames);

    /// <summary>
    /// 获取存储桶文件列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    Task<List<Item>> GetFileList(GetFileListInput input);

    /// <summary>
    /// 删除存储桶文件
    /// </summary>
    /// <param name="bucketName"> 存储桶名称 </param>
    /// <param name="objectName">文件名称</param>
    /// <param name="versionId">版本Id</param>
    /// <returns></returns>
    Task RemoveFile(string bucketName, string objectName, string? versionId = null);

    /// <summary>
    /// 获取文件下载地址
    /// </summary>
    /// <param name="bucketName"></param>
    /// <param name="objectNames"></param>
    /// <returns></returns>

    Task<string> PresignedGetObject(string bucketName, string objectNames);
}