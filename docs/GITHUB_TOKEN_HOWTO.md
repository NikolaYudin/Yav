# Где взять токен GitHub (Personal Access Token)

Токен нужен один раз — чтобы я мог отправить код в ваш репозиторий
https://github.com/NikolaYudin/Yav.git от вашего имени.

## Пошаговая инструкция (2 минуты)

1. Войдите на GitHub под аккаунтом **NikolaYudin**.
2. Откройте прямую ссылку на создание токена:
   https://github.com/settings/tokens/new
   (или: аватар в правом верхнем углу → **Settings** → слева внизу
   **Developer settings** → **Personal access tokens** →
   **Tokens (classic)** → **Generate new token (classic)**)
3. Заполните форму:
   - **Note** (название): `yav-ci-push`
   - **Expiration**: 30 days (или по вкусу)
   - **Scopes (галочки)**: отметьте только **`repo`**
     (вся группа «Full control of private repositories»). Больше не нужно.
4. Нажмите внизу **Generate token**.
5. Скопируйте появившуюся строку — она начинается с `ghp_...`
   (GitHub показывает её ОДИН раз, копируйте сразу).
6. Пришлите эту строку мне в чат следующим сообщением.

## Безопасность

- Токен даёт доступ только к вашим репозиториям (scope `repo`).
- После публикации его можно отозвать в любой момент:
  https://github.com/settings/tokens → «yav-ci-push» → **Delete/Revoke**.
- Я использую его только для `git push` в Yav.git и не записываю в файлы проекта.

## Альтернатива: Fine-grained token (ещё безопаснее)

https://github.com/settings/personal-access-tokens/new
- Repository access: **Only select repositories** → `NikolaYudin/Yav`
- Permissions → **Contents: Read and write**
