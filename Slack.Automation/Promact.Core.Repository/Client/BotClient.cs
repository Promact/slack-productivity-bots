using Promact.Core.Repository.Bot;
using Promact.Erp.Util;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Promact.Core.Repository.Bot.Rtm;

namespace Promact.Core.Repository.Client
{
    public class BotClient : IDisposable
    {
        #region Declerations

        //   private readonly ILeaveRequestRepository _leaveRepository;
        public MetaData MetaData
        {
            get
            {
                return _rtmMetaData;
            }
        }

        #region Private Variables

        /// <summary>
        /// Declare RTM Start url
        /// </summary>
        private string _urlRtmStart = StringConstant.UrlRtmStart;

        /// <summary>
        /// Declare Api Key String
        /// </summary>
        private string _strApiKey;

        /// <summary>
        /// Declare Client WebSocket
        /// </summary>
        private ClientWebSocket _webSocket;

        /// <summary>
        /// Declare Client Thread
        /// </summary>
        private Thread _clientThread = null;

        /// <summary>
        /// Declare RTM Meta Data
        /// </summary>
        private MetaData _rtmMetaData;

        #endregion

        #region Delegates
        public delegate void MessageEditEventHandler(MessageEditEventArgs e);
        public delegate void MessageEventHandler(MessageEventArgs e);
        public delegate void DataReceivedEventHandler(String data);

        #endregion

        #region Public Events

        public event MessageEventHandler Message = null;
        public event MessageEditEventHandler MesssageEdit = null;

        #endregion

        #region "Constructor"

        public BotClient(string apiKey)
        {
            _strApiKey = apiKey;
        }

        //public BotClient(ILeaveRequestRepository leaveRepository)
        //{
        //    _leaveRepository = leaveRepository;
        //}
        #endregion

        #region "Dispose Method"

        /// <summary>
        /// This method used for dispose
        /// </summary>
        public void Dispose()
        {
            DisconnectSlack();
        }

        #endregion

        #region Disconnect Method

        /// <summary>
        /// This method used for disconnect to web socket
        /// </summary>
        public void DisconnectSlack()
        {
            try
            {
                if (_webSocket != null)
                {
                    Task tsk = _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    tsk.Wait();
                    _webSocket.Dispose();
                    _webSocket = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "Connect Method"

        /// <summary>
        /// This method used for connect to client thread
        /// </summary>
        public void ConnectClientThread()
        {
            //run client on it's own thread
            try
            {
                if (_clientThread != null)
                {
                    _clientThread.Abort();
                }
                _clientThread = new Thread(new ThreadStart(ConnectSlack));
                _clientThread.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method used for connect to web socket
        /// </summary>
        private void ConnectSlack()
        {
            try
            {
                RefreshRTMMetaData();
                _webSocket = new ClientWebSocket();
                Task tsk = _webSocket.ConnectAsync(new Uri(_rtmMetaData.url), System.Threading.CancellationToken.None);
                tsk.Wait();
                ProcessMessages();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method used for fetch the RTM Meta Data
        /// </summary>
        private void RefreshRTMMetaData()
        {
            try
            {
                string strJSON = GetConnectionInfo();
                dynamic message = null;
                //   message = System.Web.Helpers.Json.Decode(strJSON);
                message = Newtonsoft.Json.JsonConvert.DeserializeObject(strJSON);
                _rtmMetaData = new MetaData(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetConnectionInfo()
        {
            try
            {
                String strURL = _urlRtmStart + "?token=" + _strApiKey;
                System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(new Uri(strURL));
                httpWebRequest.Method = "GET";
                System.Net.HttpWebResponse httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get connection info.", ex);
            }
        }

        #endregion

        #region 

        /// <summary>
        /// This method is called When Slack sends any message.
        /// </summary>
        private void ProcessMessages()
        {
            try
            {
                string strMessage;
                while (1 == 1)
                {
                    Task<string> tsk = ReadMessage();
                    tsk.Wait();
                    strMessage = tsk.Result;
                    //dynamic Data = System.Web.Helpers.Json.Decode(strMessage);
                    dynamic Data = Newtonsoft.Json.JsonConvert.DeserializeObject(strMessage);
                    switch ((String)Data.type)
                    {
                        case "message":
                            if (Data.previous_message == null)
                            {
                                MessageEventArgs messagEventArgs = new MessageEventArgs(this, Data);
                                if (Message != null)
                                {
                                    Message(messagEventArgs);
                                }
                            }
                            else
                            {
                                MessageEditEventArgs messageEditEventArgs = new MessageEditEventArgs(this, Data);
                                if (MesssageEdit != null)
                                {
                                    MesssageEdit(messageEditEventArgs);
                                }
                            }
                            break;
                        default: //null
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<string> ReadMessage()
        {
            try
            {
                ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);
                System.Net.WebSockets.WebSocketReceiveResult result = null;
                while (1 == 1)
                {
                    using (var ms = new System.IO.MemoryStream())
                    {
                        do
                        {
                            result = await _webSocket.ReceiveAsync(buffer, System.Threading.CancellationToken.None);
                            await _webSocket.SendAsync(buffer, 0, true, CancellationToken.None);
                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        } while (!result.EndOfMessage);

                        ms.Seek(0, System.IO.SeekOrigin.Begin);

                        if (result.MessageType == System.Net.WebSockets.WebSocketMessageType.Text)
                        {
                            using (var reader = new System.IO.StreamReader(ms, Encoding.UTF8))
                            {
                                // do stuff
                            }
                        }
                        Byte[] bytMessage = ms.ToArray();
                        return System.Text.ASCIIEncoding.ASCII.GetString(bytMessage);
                    }
                }
            }
            catch (System.Net.WebSockets.WebSocketException ex)
            {
                DisconnectSlack();
                throw ex;
            }
        }


        //public async void WriteMessage(string message)
        //{
        //    try
        //    {
        //        // ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);
        //        //var bytes = new Byte[8192];
        //        //bytes = Encoding.ASCII.GetBytes(message);
        //        //var len = bytes.Length;
        //        // var buffer = new ArraySegment<byte>(bytes, 0, len-1);

        //        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
        //        buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        //        await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

        //        //while (1 == 1)
        //        //{
        //        //    await _webSocket.SendAsync(buffer, WebSocketMessageType.Binary, false, CancellationToken.None);
        //        //}
        //    }
        //    catch (System.Net.WebSockets.WebSocketException ex)
        //    {
        //        //DisconnectSlack();
        //        throw ex;
        //    }
        //}

        #endregion

        #endregion

    }
}
