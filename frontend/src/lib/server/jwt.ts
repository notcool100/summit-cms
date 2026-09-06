/**
 * Decodes (does not verify) our own backend-issued JWT to read its claims for UI purposes
 * (nav visibility, permission gating). Safe because the token only ever lives in an httpOnly
 * cookie set by our own server after a real login against the backend - we never trust a token
 * that didn't just come back from `/api/auth/login` or `/api/auth/refresh`.
 */

const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const NAMEID_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
const EMAIL_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';

export interface DecodedUser {
	userId: string;
	email: string;
	roles: string[];
	permissions: string[];
	expiresAt: number; // unix seconds
}

function toArray(value: unknown): string[] {
	if (value === undefined || value === null) return [];
	return Array.isArray(value) ? value.map(String) : [String(value)];
}

export function decodeAccessToken(token: string): DecodedUser | null {
	try {
		const [, payloadB64] = token.split('.');
		const json = Buffer.from(payloadB64, 'base64url').toString('utf8');
		const payload = JSON.parse(json) as Record<string, unknown>;

		return {
			userId: String(payload[NAMEID_CLAIM] ?? payload.sub ?? ''),
			email: String(payload[EMAIL_CLAIM] ?? ''),
			roles: toArray(payload[ROLE_CLAIM]),
			permissions: toArray(payload.permission),
			expiresAt: Number(payload.exp ?? 0)
		};
	} catch {
		return null;
	}
}

export function hasPermission(user: DecodedUser | null, permission: string): boolean {
	if (!user) return false;
	return user.roles.includes('SuperAdmin') || user.permissions.includes(permission);
}
