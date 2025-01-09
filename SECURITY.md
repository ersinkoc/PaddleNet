# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |

## Reporting a Vulnerability

We take the security of PaddleNet seriously. If you believe you have found a security vulnerability, please report it to us as described below.

**Please do not report security vulnerabilities through public GitHub issues.**

Instead, please report them via email to [your-email@example.com](mailto:your-email@example.com).

You should receive a response within 48 hours. If for some reason you do not, please follow up via email to ensure we received your original message.

Please include the requested information listed below (as much as you can provide) to help us better understand the nature and scope of the possible issue:

* Type of issue (e.g. buffer overflow, SQL injection, cross-site scripting, etc.)
* Full paths of source file(s) related to the manifestation of the issue
* The location of the affected source code (tag/branch/commit or direct URL)
* Any special configuration required to reproduce the issue
* Step-by-step instructions to reproduce the issue
* Proof-of-concept or exploit code (if possible)
* Impact of the issue, including how an attacker might exploit the issue

This information will help us triage your report more quickly.

## Preferred Languages

We prefer all communications to be in English.

## Policy

We follow the principle of [Responsible Disclosure](https://en.wikipedia.org/wiki/Responsible_disclosure).

## Security Best Practices

When using PaddleNet SDK in your applications, please follow these security best practices:

1. **API Keys**
   - Never commit API keys to source control
   - Use environment variables or secure configuration management
   - Rotate API keys periodically
   - Use different API keys for sandbox and production environments

2. **Webhook Signatures**
   - Always validate webhook signatures
   - Store the public key securely
   - Use HTTPS for webhook endpoints

3. **Error Handling**
   - Don't expose detailed error messages to end users
   - Log errors securely
   - Monitor for unusual patterns

4. **Environment Separation**
   - Keep sandbox and production environments strictly separated
   - Use different API keys for each environment
   - Never use production credentials in development or testing

5. **License Keys**
   - Implement rate limiting for license validation
   - Store license keys securely
   - Monitor for unusual validation patterns

## Updates

We will advise you of security issues and updates through:
- GitHub Security Advisories
- Release notes
- Changelog updates 