using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailServerFiller.Services
{
    public sealed class EmailService
    {
        private readonly DataContext _context;

        public EmailService(DataContext context)
        {
            _context = context;
        }

        public async Task FillEmailServer()
        {
            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate("jan.fegesh@gmail.com", "zsauqvrrqfruyukm");

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                if (_context.Emails.Count() == 0)
                {
                    if (inbox != null)
                    {
                        var message = inbox.GetMessage(inbox.Count - 1);
                        _context.Emails.Add(new Email { Sender = message.From.ToString(), Subject = message.Subject, Body = message.Body.ToString(), DateRecieved = message.Date.DateTime });
                    }
                }
                else
                {
                    var lastEmail = await _context.Emails.OrderByDescending(e => e.DateRecieved).FirstOrDefaultAsync();
                    if (lastEmail != null)
                    {
                        var lastEmailDate = lastEmail.DateRecieved;

                        if (inbox != null)
                        {
                            for (var i = inbox.Count - 1; i >= 0; i--)
                            {
                                var message = inbox.GetMessage(i);

                                if (message.Date.DateTime > lastEmailDate)
                                {
                                    _context.Emails.Add(new Email { Sender = message.From.ToString(), Subject = message.Subject, Body = message.Body.ToString(), DateRecieved = message.Date.DateTime });
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                client.Disconnect(true);

                await _context.SaveChangesAsync();
            }
        }
    }
}
