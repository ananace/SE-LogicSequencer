.phony: all clean pre-publish

LogicSequencer:
	cd Data/Scripts/LogicSequencer
	dotnet build

all: LogicSequencer

pre-publish: clean
	rm -rf ../LogicSequencer
	mkdir ../LogicSequencer
	cp --reflink=always -r * ../LogicSequencer
	find ../LogicSequencer -type f -iname '*.xcf' -delete
	rm ../LogicSequencer/{Makefile,Data/Scripts/Bin64}

clean:
	find . -name 'bin' -or -name 'obj' -type d | xargs -rn1 rm -r
