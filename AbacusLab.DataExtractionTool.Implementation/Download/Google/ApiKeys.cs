using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Entitys.Google;

namespace AbacusLab.DataExtractionTool.Implementation.Download.Google
{
    internal class ApiKeys
    {
        private List<ApiKeyInfo> _apiKeyList = new List<ApiKeyInfo>();
        private static ApiKeys apiKeys;

        private ApiKeys()
        {
            InitialApiKeyList();
        }

        public static ApiKeys Keys
        {
            get { return apiKeys ?? (apiKeys = new ApiKeys()); }
        }

        public string GetApiKey()
        {
            if (_apiKeyList.All(x => x.IsUsed))
            {
                return string.Empty;
            }
            var currentKey = _apiKeyList.First(x => !x.IsUsed);
            currentKey.IsUsed = true;
            return currentKey.Key;
        }

        #region Private

        private void InitialApiKeyList()
        {
            _apiKeyList = new List<ApiKeyInfo>
                          {
                              new ApiKeyInfo{ Key = "AIzaSyBuBsGqFJYivWfkYjSoVMzjQ_U3X-dX8hc", IsUsed = false}, // Me
                              new ApiKeyInfo{ Key = "AIzaSyD2z2AxAneR_qcA3XIOykxG8V_4oTcNnx4", IsUsed = false},
                              new ApiKeyInfo{ Key = "AIzaSyBtQEi9dIU7EEa0lk4Fwop_DlmijnVVrSo", IsUsed = false},
                              new ApiKeyInfo{ Key = "AIzaSyD3jfeMZK1SWfRFDgMfxn_zrGRSjE7S8Vg", IsUsed = false},
                              new ApiKeyInfo{ Key = "AIzaSyA2-3_tlqgVuJVQ8cCvzEdAByZCQ_2OzKY", IsUsed = false}, // Muktadir
                              new ApiKeyInfo{ Key = "AIzaSyCvwGR5D-8SAhKvcJoTSTtnIvw1_N3Cfd8", IsUsed = false}, //Muktadir
                              new ApiKeyInfo{ Key = "AIzaSyBPHZcAcDW4PgG5k26Sy24DM8L2E8zzTJo", IsUsed = false}, //Muktadir
                          };
        }
        #endregion


    }
}
