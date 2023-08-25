using System;
using System.Collections.Generic;
using Ghostscript.NET;
using Ghostscript.NET.Processor;

namespace ICE.GDocs.Api.FileTools.Ghostscript
{
    public class CompressPdf
    {
        private readonly GhostscriptVersionInfo _gs_verssion_info;
        private readonly string _inputFile;
        private readonly string _outputFile;

        public string Lib
        {
            get
            {
                return Environment.CurrentDirectory + "\\FileTools\\Ghostscript";
            }
        }

        public string Dll
        {
            get
            {
                return Environment.CurrentDirectory + "\\FileTools\\Ghostscript\\DLL\\gsdll64.dll";
            }
        }

        public CompressPdf(string nameFile, string newNameFile)
        {
            this._inputFile = nameFile;
            this._outputFile = newNameFile;
            this._gs_verssion_info = new GhostscriptVersionInfo(new System.Version("9.22"), Dll, Lib, GhostscriptLicense.GPL);
        }

        public void ProcessFiles()
        {
              CompressDocument();
        }

        private void CompressDocument()
        {
            List<string> gsArgs = new List<string>();

            gsArgs.Add("-empty");
            gsArgs.Add("-dSAFER");
            gsArgs.Add("-dBATCH");
            gsArgs.Add("-dNOPAUSE");
            gsArgs.Add("-dNOPROMPT");

            gsArgs.Add("-sDEVICE=pdfwrite");
            gsArgs.Add("-dCompatibilityLevel=1.4");
            gsArgs.Add("-dPDFSETTINGS=/ebook");

            gsArgs.Add("-sOutputFile=" + _outputFile + "");
            gsArgs.Add("-f");
            gsArgs.Add(_inputFile);

            using (GhostscriptProcessor processor = new GhostscriptProcessor(_gs_verssion_info, true))
                processor.StartProcessing(gsArgs.ToArray(), null);
        }
    }
}
