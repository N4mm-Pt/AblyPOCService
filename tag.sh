# Set variables
AWS_ACCOUNT_ID=098150418900
REGION=us-east-1
REPOSITORY_NAME=backend-api

# Full ECR URI
ECR_URI="$AWS_ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/$REPOSITORY_NAME"

# Tag the image
docker tag ably-backend:latest $ECR_URI:latest