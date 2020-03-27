# BeQueue

#Quick Start


Add some Manager for execute ,

```C#
  services.AddBeQueue<DbImportManager<T>>(func =>
        {
            func.PoolSize = 1;
            func.ExecuteTimeout = TimeSpan.FromMinutes(3);
        });
````

````C#
Inject 
 public class LisListenController : Controller
    {
      
        private readonly IExecutionPool<DbImportManager<ApplicationDbContext>> _importManager;      


        /// <summary>
        /// </summary>
        /// <param name="importManager"></param>
        /// <param name="dbContext"></param>
        /// <param name="capBus"></param>
        /// <param name="logger"></param>
        public LisListenController(
            IExecutionPool<DbImportManager<ApplicationDbContext>> importManager)
        {
            _importManager = importManager;
        }
        
        [HttpPost("import")]
        public void OnLisChanged([FromBody] IEnumerable<LisCkInfo> infos)
        {
          


            var item = _importManager.Invoke(manager => { manager.Todb(infos); });
            if (item.Exception != null) _logger.LogError(item.Exception, "导入出错.");

            item.WaitResult();
        }
  
````


    


1) Service Factory like 

```C#

// MailKit
  public class SmtpClientFactory : ExecutionServiceFactory<SmtpClient>
    {
        public MailboxAddress From { get; set; }
        public string SmtpServer { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        /// <summary>
        ///     default is 25；
        /// </summary>
        public int Port { get; set; } = 25;

        public bool Ssl { get; set; }

        public override SmtpClient CreateService()
        {
            var client = new SmtpClient
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };

            client.Connect(SmtpServer, Port, Ssl);

            // Note: only needed if the SMTP server requires authentication
            client.Authenticate(UserName, Password);

            return client;
        }

        public override bool ServiceReady(SmtpClient service)
        {
            return service.IsConnected;
        }
    }



```

2) add to serviceCollection 

```C#
  service.AddBeQueueSerivce<SmtpClientFactory, SmtpClient>(config => { config.PoolSize = 2; });
```

3) get exection queue from pool
```C#

     var executItem new ExecuteItem<SmtpClient>
            {
                Action = smtp =>
                {
                   
                       smtp.Send(message);
                       return true;
                    
                }
            };

   executePool.Get().Invoke(executeItem);
   
   
   //var success = executeItem.WaitResult<bool>(); //block execute.
   
   //or
   
   successTask = Task.Run((ta)=>return  executeItem.WaitResult<bool>());
 ```C#
