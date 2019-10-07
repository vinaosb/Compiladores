run-clean: cleanup restore build run 
 
all : cleanup restore build
 
install : 
	bash -c "snap install dotnet-sdk --classic"
	bash -c "sudo snap alias dotnet-sdk.dotnet dotnet"

cleanup:
	dotnet clean LexicoSintatico/
 
restore:
	dotnet restore LexicoSintatico/
 
build:
	dotnet build LexicoSintatico/
 
run:
	dotnet run -p LexicoSintatico/LexicoSintatico.csproj
