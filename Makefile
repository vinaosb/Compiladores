run-clean: cleanup restore build run 
 
all : cleanup restore build
 
install : 
	bash -c "snap install dotnet-sdk --classic"
	bash -c "sudo snap alias dotnet-sdk.dotnet dotnet"

cleanup:
	dotnet-sdk.dotnet clean LexicoSintatico/
 
restore:
	dotnet-sdk.dotnet restore LexicoSintatico/
 
build:
	dotnet-sdk.dotnet build LexicoSintatico/
 
run:
	dotnet-sdk.dotnet run -p LexicoSintatico/LexicoSintatico.csproj
