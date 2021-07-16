using System;
using System.Linq;
using System.Text;
using TINYLib;
using static HafezLibrary.DataAccess.TinyxConfig;

namespace HafezLibrary.DataAccess.Connector
{
    public static class TinyxConnector
    {
        private static string _customerName = "حافظ";
        private static string _licenseNumber = "1234";
        private const string KeyIsConnected = "0";
        private static string TinyConnectionError { get; set; }
        private static TinyPlusCtrl TinyPlusControl { get; } = new TinyPlusCtrl();

        public static string LicenseNumber
        {
            get
            {
                // GetDataPartitions();
                return _licenseNumber;
            }
            private set => _licenseNumber = value;
        }

        public static string CustomerName
        {
            get
            {
                // GetDataPartitions();
                return _customerName;
            }
            private set => _customerName = value;
        }

        public static void FindKey()
        {
            TinyPlusControl.FindFirstTPlus(StrUserKey, StrSafeKey1, StrSafeKey2);
            TinyConnectionError = TinyPlusControl.GetTPlusErrorCode().ToString();
            GetDataPartitions();
        }

        public static bool CheckConnection()
        {
            FindKey();
            return TinyConnectionError == KeyIsConnected;
        }

        public static void GetDataPartitions()
        {
            var encodedText = GetData(DataType.Data);
            var decodedText = Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
            var dataPartitions = decodedText.Split('@').ToList();

            LicenseNumber = dataPartitions[0];
            CustomerName = dataPartitions[1];
        }

        /*public static List<string> GetDataPartitions()
        {
            var encodedText = GetData(DataType.Data);
            var decodedText = Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
            var DataPartitions = decodedText.Split('@').ToList();

            LicenseNumber = DataPartitions[0];
            CustomerName = DataPartitions[1];

            return DataPartitions;
        }*/

        public static string GetData(DataType dataType)
        {
            return dataType switch
            {
                DataType.Data => TinyPlusControl.GetTPlusData(EnumTPlusData.TPLUS_DATAPARTITION).ToString(),
                DataType.Special => TinyPlusControl.GetTPlusData(EnumTPlusData.TPLUS_SPECIALID).ToString(),
                DataType.Serial => TinyPlusControl.GetTPlusData(EnumTPlusData.TPLUS_SERIALNUMBER).ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null)
            };
        }

        public enum DataType
        {
            Data,
            Special,
            Serial
        }
    }
}