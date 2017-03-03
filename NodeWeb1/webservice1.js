var http = require('http');

var data = JSON.stringify({
  pos: 'HK',
  prodCode: 'P',
  ccy: 'HKD',
  seletedPayment: 'wirecard',
  paymentType: 'vi',
  qryDate: '2017-01-20'
});

var options = {
  host: 'pay.go.com',
  port: '80',
  path: '/PaymentWS.asmx?op=AvailCaptList',
  method: 'POST',
  headers: {
    'Content-Type': 'application/json; charset=utf-8',
    'Content-Length': data.length
  }
};

var req = http.request(options, function(res) {
  var msg = '';

  res.setEncoding('utf8');
  res.on('data', function(chunk) {
    msg += chunk;
  });
  res.on('end', function() {
    console.log(JSON.parse(msg));
  });
});

req.write(data);
req.end();