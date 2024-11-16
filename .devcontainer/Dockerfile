ARG VARIANT=1-8.0
FROM mcr.microsoft.com/devcontainers/dotnet:${VARIANT}

# Install .NET 9.0
RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh \
  && chmod +x dotnet-install.sh \
  && ./dotnet-install.sh --channel 9.0 --install-dir /usr/share/dotnet \
  && rm dotnet-install.sh
