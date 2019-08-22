using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessoftCurierPostsService
{
    static class Extensions
    {
        public static readonly string ProcDeliveries = "CDN.ProSenditNieprzetworzone";
        public static readonly string ProcPackages = "CDN.ProSenditPaczki";
        public static readonly string ProcDocuments = "CDN.ProSenditWZID";
        public static readonly string ProcUpdateSenditExt = "CDN.ProSenditExtUpdate";
        public static readonly string ColumnNamePackage = "Paczka_ID";
        public static readonly string ColumnNameDelivery = "Przesylka_ID";
        public static readonly string ColumnNameDocument = "Dokument_ID";
    }
}
