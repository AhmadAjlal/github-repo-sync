# 🚀 RepoSyncApp

Sync every repository in a GitHub organization in one command. fast, parallel, and secure.

Clone missing repos, pull updates for existing ones, automatically.

---

## ✨ Features

* 🔄 Clone or update all repos in an organization
* ⚡ Parallel processing (fast for large orgs)
* 🔐 Secure token input (hidden)
* 🌱 Supports environment variables
* 📦 Handles pagination (100+ repos)
* 💥 Resilient (continues even if some repos fail)

---

## 📸 Example

```
🚀 RepoSyncApp

GitHub Organization Name: my-org
Base Directory: C:\Repos
GitHub Token: ********

📦 Found 120 repositories

[CLONE] repo-1
[PULL]  repo-2
[CLONE] repo-3

✅ Sync complete.
```

---

## 🧰 Requirements

* .NET 6+
* Git installed
  👉 https://git-scm.com/downloads

---

## ⚙️ Usage

### Option 1 — Interactive

```bash
dotnet run
```

---

### Option 2 — Environment Variables (Recommended)

Set variables:

**Windows (PowerShell)**

```powershell
$env:GITHUB_ORG="your-org"
$env:BASE_DIR="C:\Repos"
$env:GITHUB_TOKEN="your_token_here"
```

**Linux / macOS**

```bash
export GITHUB_ORG="your-org"
export BASE_DIR="/repos"
export GITHUB_TOKEN="your_token_here"
```

Then run:

```bash
dotnet run
```

---

## 🔑 Creating a GitHub Token

1. Go to:
   https://github.com/settings/tokens

2. Click:
   **Generate new token (classic)**

3. Select scopes:

* ✅ `repo` (required for private repos)
* ✅ `read:org`

4. Generate and copy the token

---

## ⚠️ Security Tips

* Never commit your token
* Use environment variables instead of typing it
* Rotate tokens periodically

---

## 🧠 How It Works

* Calls GitHub API:

  ```
  /orgs/{org}/repos
  ```
* Clones missing repos
* Pulls updates for existing ones

---

## 🚀 Future Improvements

* [ ] Config file support
* [ ] Retry logic
* [ ] Progress bar
* [ ] Logging to file
* [ ] Support for GitHub Enterprise

---

## 🤝 Contributing

Pull requests are welcome!

---

## 📄 License

MIT
