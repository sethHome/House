using BPM.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Service
{
    public class BPMProcessModel
    {
        public static void LoadAll()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            ProcessModelCache.Instance.LoadAll(Path.Combine(basePath, "ProcessModel"));
        }
    }
}