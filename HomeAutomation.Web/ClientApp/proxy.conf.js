const target = 'http://localhost:5165';

const PROXY_CONFIG = [
  {
    context: [
      "/api/battery",
      "/api/forecast",
      "/api/invertersettings"
   ],
    proxyTimeout: 30000,
    timeout: 30000,
    target: target,
    secure: false,
    headers: {
      Connection: 'Keep-Alive'
    }
  }
]

module.exports = PROXY_CONFIG;
