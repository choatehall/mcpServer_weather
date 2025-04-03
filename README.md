# Weather MCP Server

A C# implementation of a Model Context Protocol (MCP) server that provides weather information using the National Weather Service API.

## Features

- Get weather forecasts for any location in the US using latitude and longitude
- Get active weather alerts for any US state using state codes

## Tools

### GetForecast
Get detailed weather forecast for a specific location.
- Parameters:
  - latitude: Latitude of the location
  - longitude: Longitude of the location

### GetAlerts
Get active weather alerts for a US state.
- Parameters:
  - state: Two-letter state code (e.g., CA, NY)

## Setup

1. Make sure you have .NET 8.0 or later installed
2. Clone this repository
3. Build the project: `dotnet build`
4. Run the server: `dotnet run`

## Usage

This MCP server can be used with any MCP client (like Claude for Desktop) to get weather information.

## Dependencies

- .NET 8.0
- ModelContextProtocol.Server
- System.Net.Http.Json