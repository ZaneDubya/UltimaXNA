using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Scripting;
using UltimaXNA.Diagnostics;
using UltimaXNA;
using System.IO;

namespace LuaTesting
{
    public class UIManager : Lua
    {        
        private Logger _log;
        private StringBuilder _scriptBuilder;

        public string ProcessedCode
        {
            get { return _scriptBuilder.ToString(); }
        }
            
        public UIManager()
            : base()
        {
            _log = new Logger(typeof(UIManager));
            _scriptBuilder = new StringBuilder();

            base.DoString(LuaScripts.gui_init);
            this["gui"] = this;
        }

        public void Debug(string message)
        {
            _log.Debug(message);
        }

        public void Info(string message)
        {
            _log.Info(message);
        }

        public void Warn(string message)
        {
            _log.Warn(message);
        }

        public void Error(string message)
        {
            _log.Error(message);
        }

        public void Fatal(string message)
        {
            _log.Fatal(message);
        }

        public override object[] DoString(string chunk)
        {
            _scriptBuilder.AppendLine(chunk);
            return base.DoString(chunk);
        }

        public override object[] DoString(string chunk, string chunkName)
        {
            _scriptBuilder.AppendLine(chunk);
            return base.DoString(chunk, chunkName);
        }

        public override object[] DoFile(string fileName)
        {
            _scriptBuilder.AppendLine("-- " + fileName);

            if (File.Exists(fileName))
            {
                _scriptBuilder.AppendLine(File.ReadAllText(fileName));
            }
            else
            {
                _scriptBuilder.AppendLine("-- File did not exist");
            }

            return base.DoFile(fileName);
        }
    }
}
