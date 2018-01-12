namespace Solis.Gossip.Model.Messages
{
    public class ErrorResponse : Response
    {
        private string _errorMessage;
        public ErrorResponse(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }
    }
}