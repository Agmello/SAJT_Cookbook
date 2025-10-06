const DEFAULT_API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5000";
const DEFAULT_AUTHOR_ID = process.env.NEXT_PUBLIC_DEFAULT_AUTHOR_ID ?? "";

export const environment = {
  apiBaseUrl: DEFAULT_API_BASE_URL,
  defaultAuthorId: DEFAULT_AUTHOR_ID,
};

