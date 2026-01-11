# CommandExecMcpServer
システムコマンドを実行するための MCP（Model Context Protocol）サーバーです。

## 概要
CommandExecMcpServer は、Model Context Protocol に対応した .NET ベースのサーバーで、`exec` ツールを提供します。このツールは、指定されたシステムコマンドを実行し、標準出力（stdout）と標準エラー出力（stderr）を返します。

## 機能

### exec ツール
システムコマンドを実行して、その結果を取得できます。

**パラメータ:**
- `command` (必須): 実行するコマンド名（例：`ls`、`cmd.exe`、`powershell`）
- `args` (オプション): コマンドの引数

**戻り値:**
- コマンドの実行結果を文字列で返します
- stdout のみの場合：そのまま返す
- stderr のみの場合：`[stderr]` ラベル付きで返す
- 両方ある場合：`[stdout]` と `[stderr]` ラベル付きで返す

## 必要要件
- .NET 9.0 以上
- Windows / Linux / macOS

## インストール
```bash
# プロジェクトをクローン
git clone <repository-url>
cd CommandExecMcpServer

# 依存関係をインストール
dotnet restore
```

## ビルド
### Windows
```ps1
dotnet publish CommandExecMcpServer.csproj -r win-x64 -o .\publish\win
```

### Linux
```bash
dotnet publish CommandExecMcpServer.csproj -r linux-x64 -o .\publish\linux
```

## 実行
```bash
dotnet run
```

サーバーは stdio トランスポートで起動し、MCP クライアントからの接続を待機します。

## プロジェクト構成
- `Program.cs` - MCP サーバーの初期化とツールの登録
- `CommandExecTools.cs` - コマンド実行ツール（`exec`）の実装

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

## ツール実行例 (Windows で Get-ChildItem)
```json
{"jsonrpc":"2.0","id":"3","method":"tools/call","params":{"name":"exec","arguments":{"command":"Get-ChildItem", "args": "."}}}
```

## ツール実行例 (Linux で ls -alh)
```json
{"jsonrpc":"2.0","id":"3","method":"tools/call","params":{"name":"exec","arguments":{"command":"ls", "args": "-alh"}}}
```

## 実行ファイルのダウンロード
以下のファイルがダウンロード可能です。

- CommandExecMcpServer.exe (Windows)
- CommandExecMcpServer (Linux)

ダウンロード手順は以下のドキュメントを参照してください。

[ダウンロード方法](docs/Download.md)