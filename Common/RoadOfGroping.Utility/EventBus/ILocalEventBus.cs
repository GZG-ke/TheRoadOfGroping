﻿namespace RoadOfGroping.Utility.EventBus
{
    public interface ILocalEventBus
    {
        Task PushAsync<TEto>(TEto eto) where TEto : class;

        Task EnqueueAsync<TEto>(TEto eto) where TEto : class;
    }
}