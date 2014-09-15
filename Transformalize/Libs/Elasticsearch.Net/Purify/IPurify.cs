using System;

namespace Transformalize.Libs.Elasticsearch.Net.Purify
{
    internal interface IPurifier
    {
        void Purify(Uri uri);
    }
}
