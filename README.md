# Rubbersearch

![CI](https://github.com/Ugbe/Rubbersearch/actions/workflows/ci.yml/badge.svg)
![CD](https://github.com/Ugbe/Rubbersearch/actions/workflows/cd.yml/badge.svg)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/Ugbe/Rubbersearch)
![GitHub](https://img.shields.io/github/license/Ugbe/Rubbersearch)
![GitHub issues](https://img.shields.io/github/issues/Ugbe/Rubbersearch)

Rubbersearch is a lightweight search engine and data store optimized for speed and relevance on smaller, development-scale workloads and in testing environments (production-scale coming soon). With Rubbersearch, you can perform near real-time search over large datasets, integrate with web and generative AI applications, and much more.

## Features

- Full-text search
- Autocomplete (incoming)
- Logs
- Metrics
- And more!

---

## Get Started

The simplest way to get started with Rubbersearch is to create a managed deployment with the [Rubbersearch Service on Cloud](https://rubbersearch-bdgcheg2d4gagqau.canadacentral-01.azurewebsites.net/swagger/index.html).  
The link above takes you to the Swagger UI for the REST API, where you can easily test things out.

---

## Run Rubbersearch Locally

If you prefer to install and manage Rubbersearch yourself, you can download the latest version from the [Releases page](https://www.github.com/Ugbe/Rubbersearch/releases).

> ⚠️ **Warning**  
> Do not use these instructions for production deployments (for now).  
> This setup is intended for local development and testing only.

### Download Options

- `rubbersearch-portable.zip` → use this if you already have .NET 9 installed.  
- `rubbersearch-win-x64.zip` or `rubbersearch-linux-x64.zip` → self-contained builds that don’t require .NET installed.

### Prerequisites

- **Portable build**: Requires [.NET 9.0 SDK/runtime](https://dotnet.microsoft.com/en-us/download).  
- **Self-contained builds**: No prerequisites. Just unzip and run.

### Running

After downloading and unzipping, run:

```bash
dotnet Rubbersearch.Api.dll
````

You can then access Rubbersearch at:
`http://localhost:5298`

---
## API Access

Before indexing or searching, you’ll need an **API key**.
This is generated via the `RequestKey` endpoint.

Each API key maps to a **tenant**. A tenant has a single index, which could represent an app, a module, or any logical grouping. You can create multiple tenants (and thus multiple indexes) depending on your needs.
---

## Sending Requests to Rubbersearch

You send data and queries through REST APIs.
You can use [Postman](https://www.postman.com), [curl](https://curl.se), or your backend code.

---

### Index Data

You add data to Rubbersearch by sending JSON documents.

**Document format:**

```json
{
  "docId": "string",
  "title": "string",
  "content": "string",
  "url": "string"
}
```

> ℹ️ Notes:
>
> * `docId`: The ID of the document in your system/DB.
> * `title`: Display title of the document.
> * `url`: URL in your system where the document can be accessed.
> * `content`: The raw text of the document.

**Example:**

```http
POST /api/index
{
  "docId": "doc-123",
  "title": "Optimising Search by Jimmy Kimmel",
  "content": "The exponential growth of online content has led to significant challenges in information retrieval...",
  "url": "herewego.websites.com/resources/books/doc-123"
}
```

This request:

* Stores the document JSON,
* Tokenizes the title and content,
* Updates the tenant’s index.

---

### Search Data

Indexed documents are searchable in near real-time.

**Example search:**

```http
GET /api/search
{
  "query": "online searching speed",
  "max": 10
}
```

Or via POST:

```http
POST /api/search
{
  "query": "online searching speed",
  "max": 10
}
```

**Curl examples:**

```bash
curl -X 'GET' \
  'https://rubbersearch-bdgcheg2d4gagqau.canadacentral-01.azurewebsites.net/api/Search?q=online%20searching%20speed&max=10' \
  -H 'accept: */*' \
  -H 'Authorization: Bearer 0xn1c31itt1eAPlk3yyeahyeah--+'
```

```bash
curl -X 'POST' \
  'https://rubbersearch-bdgcheg2d4gagqau.canadacentral-01.azurewebsites.net/api/Search' \
  -H 'accept: */*' \
  -H 'Authorization: Bearer 0xn1c31itt1eAPlk3yyeahyeah--+' \
  -H 'Content-Type: application/json' \
  -d '{
    "q": "online searching speed",
    "max": 10
  }'
```

---

## Documentation

For the complete Rubbersearch documentation, including design choices, algorithms, and optimizations, visit:
👉 \[Documentation link coming soon]

---

## Examples and Guides

Tutorials, guides, and videos are in progress.
In the meantime, feel free to reach out to any contributor for help.

---

## Questions, Problems, or Suggestions?

* Report bugs or request features: [Open a GitHub Issue](https://github.com/Ugbe/Rubbersearch/issues/new/choose). Please check existing issues first.
* Need help using Rubbersearch? Reach out to me or any contributor via our GitHub profiles.
* Contributions are welcome! Fork the repo, open PRs, or share ideas. This project grows with the community.

---
