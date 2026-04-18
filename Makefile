up:
	docker compose up

# This leaves containers in a "Stopped" state
stop:
	docker compose stop

# removes the containers entirely
down:
	docker compose down

run-dockdev:
	docker compose -f docker-compose.dev.yml up

# First run DB
run-db:
	docker compose up siaru.db

# Second run web
run-web:
	dotnet watch run --project SIARU.Web
