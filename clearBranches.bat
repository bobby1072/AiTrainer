@echo off
git fetch -pP
for /F "tokens=1" %%B in ('git branch -vv ^| find ": gone]"') do (
    git branch -D %%B
)