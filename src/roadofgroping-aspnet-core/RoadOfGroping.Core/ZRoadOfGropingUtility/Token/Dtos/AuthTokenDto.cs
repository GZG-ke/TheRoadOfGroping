﻿namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Token.Dtos
{
    public class AuthTokenDto
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime Expiration { get; set; }
    }
}