ROOT	:= $(abspath $(dir $(lastword $(MAKEFILE_LIST))))
NAME	:= watchingwatches
TAG 	:= tylerrichey/$(NAME)

.PHONY: all build

all: build

build:
	@docker build -t $(TAG) $(ROOT)

run:
	@docker run -it -d --restart always -v "/home/teeman/watchingwatches":/app/db -p 10.0.0.200:5005:5005 --name $(NAME) $(TAG) 

clean:
	@docker stop $(NAME)
	@docker rm -v $(NAME)
	@docker rmi -f $(TAG)
