# Unsloth AI
https://unsloth.ai/

## Default

## Laya
https://unsloth.ai/docs/models/decision-laya

### Laya Docker
https://github.com/pambrose/laya-server/blob/master/README.md

``` cmd
docker run --rm -p 8888:8000 -v laya-checkpoints:/home/app/.cache/huggingface -e LAYA_SERVER_PRELOAD=english pambrose/laya-server:latest
```

# Ollama

## nimble
```
ollama run nimble
```

---

# llama

## Lev
https://huggingface.co/ggml-org/lev-GGUF

``` cmd
llama serve -hf ggml-org/lev-GGUF
```

## OpenJev
https://huggingface.co/ggml-org/OpenJev-GGUF

``` cmd
llama serve -hf ggml-org/OpenJev-GGUF
``` 

## Kev
https://huggingface.co/ggml-org/Kev-4B-GGUF

``` cmd
llama-server -hf ggml-org/Kev-4B-GGUF
``` 

## Laya
https://huggingface.co/ggml-org/Laya-GGUF

``` cmd
llama serve -hf ggml-org/Laya-GGUF
``` 

## Julia-1
https://huggingface.co/ggml-org/Julia-1-GGUF

``` cmd
llama serve -hf ggml-org/Julia-1-GGUF
``` 