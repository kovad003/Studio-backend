TAG := studio-backend

init:
	ASPNETCORE_ENVIRONMENT=Production dotnet-ef migrations add PostgresInitial -p Persistence -s API
	ASPNETCORE_ENVIRONMENT=Production dotnet-ef database update -p Persistence -s API

docker-run:
	docker run --rm -it \
		-p 8080:80 \
		${TAG}

docker-build:
	docker build . -t ${TAG}

run:
	dotnet run --project API

rerun: publish
	cd out/ && dotnet API.dll

dev-rerun: publish
	cd out/ && ASPNETCORE_ENVIRONMENT=Development dotnet API.dll

restore:
	dotnet restore "Studio-backend.sln"

publish: clean
	dotnet publish -c Release -o out

clean: clean-out clean-migration
	
clean-out:
	find . -iname bin -o -iname obj -o -iname out | xargs -I{} rm -rf "{}"

clean-migration:
	ASPNETCORE_ENVIRONMENT=Production dotnet-ef database update 0 -p Persistence -s API
	ASPNETCORE_ENVIRONMENT=Production dotnet-ef migrations remove -p Persistence -s API

