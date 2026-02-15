FROM cbizappdev-c4erg2gbekgvfxhd.azurecr.io/library/cert:1.0.0 AS cert
 
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
 
COPY . .
 
COPY --from=cert ./*.crt /usr/local/share/ca-certificates/

RUN apt-get update \
	&& apt-get install -y ca-certificates \
	&& update-ca-certificates \
	&& apt-get install krb5-user cifs-utils -y \
	&& ln -fs /usr/share/zoneinfo/America/New_York /etc/localtime \
	&& dpkg-reconfigure --frontend noninteractive tzdata \
	&& apt-get clean \
	&& mkdir -p /app/logs

USER 100000 

ENTRYPOINT ["dotnet", "/app/Marcum.CCH.Axcess.API.dll"]
