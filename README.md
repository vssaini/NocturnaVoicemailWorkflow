
# Nocturna Voicemail Workflow

**Nocturna Voicemail Workflow** is an Azure Durable Function-based webhook designed to receive **RingCentral voicemail transcription events** and persist the data to both **Azure SQL Database** and **file storage**.

## 🚀 Project Overview

### What Problem Does It Solve?
This function automates the process of storing voicemail's transcriptions received via RingCentral into structured storage, enabling faster access, tracking, and auditing.

### Target Audience
**Nocturna Sleep** support and development teams who need to manage and troubleshoot voicemail records efficiently.

---

## 🛠 Tech Stack

- **Platform**: Azure Function App (HTTP Trigger)
- **Language**: .NET 8
- **Database**: Azure SQL Server (via Dapper)
- **Logging**: Serilog
- **File Storage**: Server accessible over FTP
- **Integration**: RingCentral Webhook

---

## 📦 Installation Guide

### Prerequisites
Ensure the following are installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Azure CLI (for deployment and configuration)
- Git

### Steps to Clone, Build, and Run Locally
```sh
# Clone the repository
git clone https://github.com/Imagine-Nation-Software/NocturnaVoicemail.git

# Navigate to the project directory
cd NocturnaVoicemail

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run locally
func start
```

### Generate RingCentral Subscription
Read the [Nocturna Voicemail Instructions](https://docs.google.com/document/d/14fUd6-mHUbkPdSmL-ixPimLP_EsQo6Vw3EeWtyX88hI/edit?usp=sharing) doc for generating subscription and validation-token.

---

## 📖 How It Works

1. **RingCentral** sends a webhook with voicemail and transcription data to the Azure Function endpoint.
2. The function parses and validates the payload.
3. It saves relevant details (caller, recipient, transcription text, voicemail URL) into the **Azure SQL Database**.
4. It writes the transcription and metadata into a excel file.

---

## ⚙️ Configuration

Update the following settings in your `appsettings.json` or Azure App Configuration:

```json
{
  "RingCentral": {
    "ServerUrl": "https://platform.ringcentral.com",
    "MediaUrl": "https://media.ringcentral.com",
    "ClientId": "",
    "ClientSecret": "",
    "JwtToken": "",
    "WebhookSecret": "" // validation-token
  },

  "Ftp": {
    "Hostname": "srv.nocturnaportal.com",
    "Username": "nocturnapftp",
    "Password": "",
    "RootDirectory": "/httpdocs/database",
    "ExcelFileName": "Voicemail_Transcription.xlsx",
    "LogToConsole": false,
    "ReadTimeoutInSeconds": 15
  },

  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

---

## ✨ Features

- Receives and validates **RingCentral webhook events**.
- Logs incoming data using **Serilog**.
- Saves **Webhook Payloads** and **transcriptions** to Azure SQL Database.
- Supports file-based backup of voicemail transcriptions for later use.

---

## 🤝 Contributing

This project does not accept contributions at this time.

---

## 📜 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

🔗 **To contact developer, visit**: [LinkedIn Profile](https://www.linkedin.com/in/vssaini/)
