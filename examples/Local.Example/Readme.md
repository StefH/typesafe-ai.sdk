## laya-server
https://github.com/pambrose/laya-server/blob/master/README.md

## commands

``` cmd
docker run --rm -p 8000:8000 -v laya-checkpoints:/home/app/.cache/huggingface -e LAYA_SERVER_PRELOAD=english pambrose/laya-server:latest
```