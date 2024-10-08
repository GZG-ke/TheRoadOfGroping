﻿namespace RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.SignalR.Dtos
{
    public class MessageInput
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户ID列表
        /// </summary>
        public List<string> UserIds { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
    }
}