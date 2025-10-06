import { api } from "@/lib/api-client";
import type { UserSummary } from "@/features/users/types";

export async function getUsers(signal?: AbortSignal): Promise<UserSummary[]> {
  return api.get("api/users", { signal }).json<UserSummary[]>();
}
