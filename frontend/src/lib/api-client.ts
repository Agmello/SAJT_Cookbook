import ky from "ky";

import { environment } from "@/lib/env";

const api = ky.create({
  prefixUrl: environment.apiBaseUrl,
  retry: {
    limit: 1,
  },
  headers: {
    "Content-Type": "application/json",
  },
});

export { api };
