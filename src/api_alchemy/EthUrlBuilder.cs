using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_alchemy
{
    public static class EthUrlBuilder
    {

        public static string getBlockByNumber()
        {
            var res = """
                                {
                    "jsonrpc":"2.0",
                    "method":"eth_getBlockByNumber",
                    "params":["0x1b4", true],
                    "id":0
                }
                """;

            return res;
        }
    }
}
