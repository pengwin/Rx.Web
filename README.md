# Rx.Web

Simple Http server implemenatation. Based on [Rx.Net](https://github.com/dotnet/reactive)

## Perf

```
ivan@pop-os:~$ ab -n 1000 -c 100 http://localhost:5000/
This is ApacheBench, Version 2.3 <$Revision: 1879490 $>
Copyright 1996 Adam Twiss, Zeus Technology Ltd, http://www.zeustech.net/
Licensed to The Apache Software Foundation, http://www.apache.org/

Benchmarking localhost (be patient)
Completed 100 requests
Completed 200 requests
Completed 300 requests
Completed 400 requests
Completed 500 requests
Completed 600 requests
Completed 700 requests
Completed 800 requests
Completed 900 requests
Completed 1000 requests
Finished 1000 requests


Server Software:        RX.WEB
Server Hostname:        localhost
Server Port:            5000

Document Path:          /
Document Length:        2 bytes

Concurrency Level:      100
Time taken for tests:   0.061 seconds
Complete requests:      1000
Failed requests:        0
Total transferred:      78000 bytes
HTML transferred:       2000 bytes
Requests per second:    16294.87 [#/sec] (mean)
Time per request:       6.137 [ms] (mean)
Time per request:       0.061 [ms] (mean, across all concurrent requests)
Transfer rate:          1241.21 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    2   0.6      2       4
Processing:     0    4   1.5      4      11
Waiting:        0    3   1.6      3      11
Total:          0    6   1.6      6      13

Percentage of the requests served within a certain time (ms)
  50%      6
  66%      6
  75%      6
  80%      6
  90%      7
  95%      8
  98%     11
  99%     13
 100%     13 (longest request)
```