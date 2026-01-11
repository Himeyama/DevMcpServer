# DevMcpServer
開発ツールを支援する MCP（Model Context Protocol）サーバーです。

## 概要
DevMcpServer は、Model Context Protocol に対応した .NET ベースのサーバーで、開発ツールを提供します。

## 機能

## ツール一覧と使い方

以下は各ツールの説明、入力スキーマ、利用例です。

1. now
 - 説明: 現地時刻をフォーマットして返す（例: `Mon Jan 12 2026 07:25:23 GMT+0900`）。
 - 引数: なし (`{}`)
 - 例リクエスト:
   ```
   {"jsonrpc":"2.0","id":"1","method":"tools/call","params":{"name":"now","arguments":{}}}
   ```
 - 例レスポンス:
   ```
   {"result":{"content":[{"type":"text","text":"Mon Jan 12 2026 07:38:41 GMT+0900"}]},"id":"1","jsonrpc":"2.0"}
   ```

2. unixtime
 - 説明: 現在の Unix 時刻（秒）を返す。
 - 引数: なし
 - 例:
   ```
   {"jsonrpc":"2.0","id":"2","method":"tools/call","params":{"name":"unixtime","arguments":{}}}
   ```

3. uuid
 - 説明: 新しい UUID（GUID）を生成して返す。
 - 引数: なし

4. randpass
 - 説明: ランダムなパスワードを生成（文字セット: 0-9a-zA-Z）。
 - 引数:
   - `length` (integer, optional, default: 12)
 - 例:
   ```
   {"jsonrpc":"2.0","id":"3","method":"tools/call","params":{"name":"randpass","arguments":{}}}
   {"jsonrpc":"2.0","id":"4","method":"tools/call","params":{"name":"randpass","arguments":{"length":20}}}
   ```

5. exec
 - 説明: システムコマンドを実行（Windows: PowerShell、Linux: Bash）。標準出力（および実装次第でエラー情報）を返す。
 - 引数:
   - `command` (string) — 実行するコマンド（例: `ls`, `echo`）
   - `args` (string, optional) — 引数文字列
 - 例:
   ```
   {"jsonrpc":"2.0","id":"5","method":"tools/call","params":{"name":"exec","arguments":{"command":"echo","args":"Hello from test"}}}
   ```
 - 注意:
   - Windows の PowerShell 環境では `echo` に複数引数を渡すと改行で分けて出力されることがあります。単一行で出力したい場合は引数をクォートするなど工夫してください（例: `"args":"\"Hello from test\""`）。

6. b64enc / b64dec
 - b64enc
   - 説明: UTF-8 で Base64 エンコード。
   - 引数: `input` (string)
   - 例:
     ```
     {"jsonrpc":"2.0","id":"6","method":"tools/call","params":{"name":"b64enc","arguments":{"input":"Hello World!"}}}
     ```
 - b64dec
   - 説明: Base64 をデコードして UTF-8 文字列に戻す。
   - 引数: `base64` (string)
   - 例:
     ```
     {"jsonrpc":"2.0","id":"7","method":"tools/call","params":{"name":"b64dec","arguments":{"base64":"SGVsbG8gV29ybGQh"}}}
     ```
   - 注意:
     - 無効な Base64 文字列を渡すと例外になり、エラーが返ります。例:
       ```
       {"jsonrpc":"2.0","id":"8","method":"tools/call","params":{"name":"b64dec","arguments":{"base64":"!!!invalid!!!"}}}
       ```
       - この場合ログに例外スタックトレースが出力され、レスポンスにはエラー表現が返されます（実装依存）。

7. htmlenc / htmldec
 - htmlenc
   - 説明: `<`, `>`, `&`, `"` 等を HTML エスケープする。
   - 引数: `input` (string)
   - 例:
     ```
     {"jsonrpc":"2.0","id":"9","method":"tools/call","params":{"name":"htmlenc","arguments":{"input":"<script>alert(\"xss\")</script>"}}}
     ```
     - 結果例: `&lt;script&gt;alert(&quot;xss&quot;)&lt;/script&gt;`
 - htmldec
   - 説明: HTML エンティティをデコードする。
   - 引数: `input` (string)
   - 例:
     ```
     {"jsonrpc":"2.0","id":"10","method":"tools/call","params":{"name":"htmldec","arguments":{"input":"&lt;div&gt;Hello &amp; Welcome&lt;/div&gt;"}}}
     ```
     - 注意: JSON シリアライズ過程で `<` や `>` が `\u003C` / `\u003E` のようにエスケープされることがあります（意味的には同値）。

8. urlenc / urldec
 - urlenc
   - 説明: URL エンコード（UTF-8、パーセントエンコーディング）。実装は application/x-www-form-urlencoded スタイルを使う場合、空白が `+` になります。
   - 引数: `input` (string)
   - 例:
     ```
     {"jsonrpc":"2.0","id":"11","method":"tools/call","params":{"name":"urlenc","arguments":{"input":"Hello, world"}}}
     ```
     - 例結果: `Hello%2C+world` （空白が `+`）
   - 空白を `%20` にしたい場合は別実装が必要です。
 - urldec
   - 説明: パーセントデコード。
   - 引数: `input` (string)
   - 例:
     ```
     {"jsonrpc":"2.0","id":"12","method":"tools/call","params":{"name":"urldec","arguments":{"input":"%E3%81%82%20test"}}}
     ```
     - 結果例: `あ test`

9. hexenc / hexdec
 - hexenc
   - 説明: 入力文字列を UTF-8 バイト列として 16 進表現に変換。
   - 引数:
     - `input` (string)
     - `upperCase` (boolean, optional, default: false) — 大文字フォーマットを使うか
   - 例:
     ```
     {"jsonrpc":"2.0","id":"13","method":"tools/call","params":{"name":"hexenc","arguments":{"input":"おはよう","upperCase":true}}}
     ```
     - 結果例: `E3818AE381AFE38288E38186`
 - hexdec
   - 説明: 16 進文字列を UTF-8 としてデコードして文字列に戻す。入力の空白は許容されることがある。
   - 引数: `hex` (string)
   - 例:
     ```
     {"jsonrpc":"2.0","id":"14","method":"tools/call","params":{"name":"hexdec","arguments":{"hex":"e3818ae381afe38288e38186"}}}
     ```
     - 結果例: `おはよう`

10. hash
  - 説明: 文字列のハッシュを計算して 16 進文字列で返す。
  - 引数:
    - `input` (string)
    - `algorithm` (string, optional, default: "sha256") — `md5`, `sha1`, `sha256`, `sha512`
  - 例:
    ```
    {"jsonrpc":"2.0","id":"15","method":"tools/call","params":{"name":"hash","arguments":{"input":"password"}}}
    {"jsonrpc":"2.0","id":"16","method":"tools/call","params":{"name":"hash","arguments":{"input":"password","algorithm":"md5"}}}
    ```
    - `sha256` 結果例: `5e884898...`
    - `md5` 結果例: `5f4dcc3b5aa765d61d8327deb882cf99`

11. dummy
  - 説明: 固定の Lorem Ipsum テキストを返す（テスト用途）。
  - 引数:
    - `length` (integer, optional, default: 200) — 返す文字数（実装上の最大に注意）
  - 例:
    ```
    {"jsonrpc":"2.0","id":"17","method":"tools/call","params":{"name":"dummy","arguments":{"length":100}}}
    ```

## 必要要件
- .NET 9.0 以上
- Windows / Linux / macOS

## インストール
```bash
# プロジェクトをクローン
git clone <repository-url>
cd DevMcpServer

# 依存関係をインストール
dotnet restore
```

## ビルド
### Windows
```ps1
dotnet publish DevMcpServer.csproj -r win-x64 -o .\publish\win
```

### Linux
```bash
dotnet publish DevMcpServer.csproj -r linux-x64 -o .\publish\linux
```

## 実行
```bash
dotnet run
```

サーバーは stdio トランスポートで起動し、MCP クライアントからの接続を待機します。

## プロジェクト構成
- `Program.cs` - MCP サーバーの初期化とツールの登録
- `CommandExecTools.cs` - コマンド実行ツール（`exec`）の実装
- `DevTools.cs` - 開発ツールの実装

## 使用例
MCP クライアントから以下のように `exec` ツールを呼び出せます：

### MCP サーバーの初期化
```json
{"jsonrpc":"2.0","method":"initialize","id":"1","params":{"protocolVersion":"2024-11-05","clientInfo":{"name":"example-client","version":"0.0.1"},"capabilities":{}}}
```

### ツール一覧の取得
```json
{"jsonrpc": "2.0", "id": "2", "method": "tools/list"}
```

### MCP 実行テスト
[TEST.md](TEST.md) を参照してください。

## 実行ファイルのダウンロード
以下のファイルがダウンロード可能です。

- DevMcpServer.exe (Windows)
- DevMcpServer (Linux)

ダウンロード手順は以下のドキュメントを参照してください。

[ダウンロード方法](docs/Download.md)