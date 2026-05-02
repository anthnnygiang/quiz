project := "Quiz.csproj"
configuration := "Release"
package := "AnthonyGiang.Quiz"
source := "./artifacts"

default:
    @just --list

build:
    dotnet build {{project}} -c {{configuration}}

pack: build
    dotnet pack {{project}} -c {{configuration}} --no-build

install: pack
    -dotnet tool uninstall --global {{package}}
    dotnet tool install --global --add-source {{source}} --no-cache {{package}}

reinstall: install
