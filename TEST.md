# テストケース
> 現在時刻の取得
```json
{"jsonrpc":"2.0","id":"11","method":"tools/call","params":{"name":"now","arguments":{}}}
```

> パスワード作成 (12文字)
```json
{"jsonrpc":"2.0","id":"15","method":"tools/call","params":{"name":"randpass","arguments":{}}}
```

> パスワード作成
```json
{"jsonrpc":"2.0","id":"16","method":"tools/call","params":{"name":"randpass","arguments":{"length":20}}}
```

> コマンド実行
```json
{"jsonrpc":"2.0","id":"17","method":"tools/call","params":{"name":"exec","arguments":{"command":"echo","args":"Hello from test"}}}
```

> Base64 エンコード
```json
{"jsonrpc":"2.0","id":"18","method":"tools/call","params":{"name":"b64enc","arguments":{"input":"Hello World!"}}}
```

> Base64 デコード
```json
{"jsonrpc":"2.0","id":"12","method":"tools/call","params":{"name":"b64dec","arguments":{"base64":"SGVsbG8gV29ybGQh"}}}
```

> Base64 デコード
```json
{"jsonrpc":"2.0","id":"13","method":"tools/call","params":{"name":"b64dec","arguments":{"base64":"!!!invalid!!!"}}}
```

> HTML エンコード
```json
{"jsonrpc":"2.0","id":"19","method":"tools/call","params":{"name":"htmlenc","arguments":{"input":"<script>alert(\"xss\")</script>"}}}
```

> HTML デコード
```json
{"jsonrpc":"2.0","id":"10","method":"tools/call","params":{"name":"htmldec","arguments":{"input":"&lt;div&gt;Hello &amp; Welcome&lt;/div&gt;"}}}
```

> URL エンコード
```json
{"jsonrpc":"2.0","id":"20","method":"tools/call","params":{"name":"urlenc","arguments":{"input":"Hello, world"}}}
```

> URL デコード
```json
{"jsonrpc":"2.0","id":"14","method":"tools/call","params":{"name":"urldec","arguments":{"input":"%E3%81%82%20test"}}}
```

> UNIX 時刻取得
```json
{"jsonrpc":"2.0","id":"22","method":"tools/call","params":{"name":"unixtime","arguments":{}}}
```

> SHA256
```json
{"jsonrpc":"2.0","id":"23","method":"tools/call","params":{"name":"uuid","arguments":{}}}
```

> ハッシュ化
```json
{"jsonrpc":"2.0","id":"24","method":"tools/call","params":{"name":"hash","arguments":{"input":"password"}}}
```

> ハッシュ化 (MD5)
```json
{"jsonrpc":"2.0","id":"25","method":"tools/call","params":{"name":"hash","arguments":{"input":"password","algorithm":"md5"}}}
```

> ダミーテキストの作成
```json
{"jsonrpc":"2.0","id":"26","method":"tools/call","params":{"name":"dummy","arguments":{"length":100}}}
```

> HEX エンコード
```json
{"jsonrpc":"2.0","id":"27","method":"tools/call","params":{"name":"hexenc","arguments":{"input":"おはよう","upperCase":true}}}
```

> HEX デコード
```json
{"jsonrpc":"2.0","id":"21","method":"tools/call","params":{"name":"hexdec","arguments":{"hex":"e3818ae381afe38288e38186"}}}
```