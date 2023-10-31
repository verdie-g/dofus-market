# Infrastructure

## Requirements

- Linux or Windows with [WSL](https://docs.microsoft.com/en-us/windows/wsl/install-win10)
- [ansible](https://docs.ansible.com/ansible/latest/installation_guide/intro_installation.html)

## Run

- upload your SSH public key to all hosts
- `ANSIBLE_CONFIG=./ansible.cfg ansible-playbook playbook.yml`

## Encrypt/Decrypt secret

`ansible-vault encrypt_string --vault-password-file ./secret`

`ANSIBLE_CONFIG=./ansible.cfg ansible all -m debug -a 'var=my_secret'`
