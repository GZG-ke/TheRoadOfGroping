﻿namespace RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus
{
    public class LocalEventBus : ILocalEventBus
    {
        private readonly IConcurrentDictionaryBasedEventHandlerManager _eventHandlerManager;

        public LocalEventBus(IConcurrentDictionaryBasedEventHandlerManager eventHandlerManager)
        {
            _eventHandlerManager = eventHandlerManager;
        }

        /// <summary>
        /// 写入队列，后台执行事件
        /// </summary>
        /// <typeparam name="TEto"></typeparam>
        /// <param name="eto"></param>
        /// <returns></returns>
        public async Task EnqueueAsync<TEto>(TEto eto)
            where TEto : class
        {
            await _eventHandlerManager.BackgroundExecuteAsync(eto);
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="TEto"></typeparam>
        /// <param name="eto"></param>
        /// <returns></returns>
        public async Task PushAsync<TEto>(TEto eto)
            where TEto : class
        {
            await _eventHandlerManager.ExecuteAsync(eto);
        }
    }
}