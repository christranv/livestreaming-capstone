namespace Stream.API.Application.Models
{
    public class SrsCallbackModel
    {
        public string Action { get; private init; }
        public string Ip { get; private init; }
        public string Vhost { get; private init; }
        public string App { get; private init; }
        public string TcUrl { get; private init; }
        public string Stream { get; private init; }
        public string Param { get; private init; }

        public SrsCallbackModel(string action, string ip, string vhost, string app, string tcUrl,
            string stream, string param)
        {
            Action = action;
            Ip = ip;
            Vhost = vhost;
            App = app;
            TcUrl = tcUrl;
            Stream = stream;
            Param = param;
        }

        public override string ToString()
        {
            return
                $"{nameof(Action)}: {Action}, {nameof(Ip)}: {Ip}, {nameof(Vhost)}: {Vhost}, {nameof(App)}: {App}, {nameof(TcUrl)}: {TcUrl}, {nameof(Stream)}: {Stream}, {nameof(Param)}: {Param}";
        }
    }
}