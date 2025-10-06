import { useQuery, type UseQueryOptions } from "@tanstack/react-query";

import { getUsers } from "@/features/users/api/get-users";
import type { UserSummary } from "@/features/users/types";

const USERS_QUERY_KEY = ["users"];

export function useUsersQuery(options?: Pick<UseQueryOptions<UserSummary[], Error>, "enabled">) {
  return useQuery<UserSummary[], Error>({
    queryKey: USERS_QUERY_KEY,
    queryFn: ({ signal }) => getUsers(signal),
    staleTime: 1000 * 60,
    ...options,
  });
}
