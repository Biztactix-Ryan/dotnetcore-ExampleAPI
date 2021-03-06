﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Contracts.V2
{
    public static class APIRoutes
    {
        private const string apiVersion = "v2";
        public static class Weather
        {
            public const string Get = "api/" + apiVersion + "/weather";
            public const string AuthGet = "api/" + apiVersion + "/weather/auth";
        }
        public static class Auth
        {
            public const string AuthUserLogin = "api/" + apiVersion + "/auth/UserLogin";
            public const string AuthAPILogin = "api/" + apiVersion + "/auth/APILogin";

        }
    }
}
