package com.thienbao.pharmacymanagementauth.service;

import com.thienbao.pharmacymanagementauth.constant.WarningConstant;
import com.thienbao.pharmacymanagementauth.dto.request.SignUpRequestDto;
import com.thienbao.pharmacymanagementauth.dto.request.SignUpResponseDto;
import com.thienbao.pharmacymanagementauth.dto.response.UserResponseDto;
import com.thienbao.pharmacymanagementauth.dto.signin.SignInRequestDto;
import com.thienbao.pharmacymanagementauth.dto.signin.SignInResponseDto;
import com.thienbao.pharmacymanagementauth.dto.signout.SignOutResponseDto;
import com.thienbao.pharmacymanagementauth.entity.InvalidatedAccessToken;
import com.thienbao.pharmacymanagementauth.entity.RefreshToken;
import com.thienbao.pharmacymanagementauth.entity.User;
import com.thienbao.pharmacymanagementauth.exception.entity.GlobalException;
import com.thienbao.pharmacymanagementauth.mapper.UserMapper;
import lombok.AccessLevel;
import lombok.RequiredArgsConstructor;
import lombok.experimental.FieldDefaults;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.AuthenticationException;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.oauth2.jwt.Jwt;
import org.springframework.security.oauth2.server.resource.authentication.JwtAuthenticationToken;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@SuppressWarnings({WarningConstant.SPELL_CHECKING_INSPECTION})
@FieldDefaults(level = AccessLevel.PRIVATE, makeFinal = true)
@RequiredArgsConstructor
@Service
public class AuthService {

	AuthenticationManager authenticationManager;
	InvalidatedAccessTokenService invalidatedAccessTokenService;
	RefreshTokenService refreshTokenService;
	UserService userService;
	UserMapper userMapper;
	JwtService jwtService;

	/**
	 * SignIn
	 */

	public SignInResponseDto signInInternal(SignInRequestDto request) {
		authenticate(request.getUsername(), request.getPassword());

		User user = userService.findByUsernameOrThrow(request.getUsername());

		return signIn(user.getId(), user.getUsername());
	}

	private void authenticate(String username, String password) {
		UsernamePasswordAuthenticationToken usernamePasswordAuthenticationToken
				= new UsernamePasswordAuthenticationToken(username, password);
		authenticationManager.authenticate(usernamePasswordAuthenticationToken);
	}

	private SignInResponseDto signIn(
			Long userId,
			String username
	) {
		String accessToken = jwtService.generateAccessToken(userId, username);
		String refreshToken = jwtService.generateRefreshToken(userId, username);

		refreshTokenService.save(
				RefreshToken.builder()
						.jti(jwtService.extractJtiFromRefreshToken(refreshToken))
						.expiryDate(jwtService.extractExpiryFromRefreshToken(refreshToken))
						.build()
		);

		return SignInResponseDto.builder()
				.accessToken(accessToken)
				.refreshToken(refreshToken)
				.build();
	}

	public SignUpResponseDto signUpInternal(SignUpRequestDto request) {
		return signUp(request);
	}

	private SignUpResponseDto signUp(SignUpRequestDto request) {
		User newUser = userService.createUser(request);

		return SignUpResponseDto.builder()
				.userId(newUser.getId())
				.username(newUser.getUsername())
				.build();
	}

	public SignOutResponseDto signOut(
			Authentication authentication,
			String rawRefreshToken
	) {
		try {
			Jwt refreshTokenJwt = null;
			if (rawRefreshToken != null && !rawRefreshToken.isEmpty()) {
				refreshTokenJwt = jwtService.extractJwtFromRefreshToken(rawRefreshToken);
				refreshTokenService.revokeByJti(jwtService.extractJtiFromJwt(refreshTokenJwt));
			}

			Jwt accessTokenJwt = jwtService.extractJwtFromAuthentication(authentication);

			InvalidatedAccessToken newInvalidatedAccessToken = InvalidatedAccessToken.builder()
					.jti(jwtService.extractJtiFromJwt(accessTokenJwt))
					.expiryDate(jwtService.extractExpiryFromJwt(accessTokenJwt))
					.build();
			invalidatedAccessTokenService.save(newInvalidatedAccessToken);

			if (refreshTokenJwt == null) {
				return SignOutResponseDto.builder()
						.signedOutAccessToken(jwtService.extractTokenFromJwt(accessTokenJwt))
						.build();
			} else {
				return SignOutResponseDto.builder()
						.signedOutAccessToken(jwtService.extractTokenFromJwt(accessTokenJwt))
						.signedOutRefreshToken(jwtService.extractTokenFromJwt(refreshTokenJwt))
						.build();
			}
		} catch (Exception e) {
			throw new GlobalException(GlobalException.Type.INTERNAL_SERVER_ERROR, "Internal server error");
		}
	}

	public Authentication getAuthentication() {
		Authentication authentication = SecurityContextHolder.getContext().getAuthentication();

		if (!(authentication instanceof JwtAuthenticationToken)) {
			throw new GlobalException(GlobalException.Type.INVALID_AUTHENTICATION, "Unauthorized");
		}
		String jti = jwtService.extractJtiFromAuthentication(authentication);
		if (invalidatedAccessTokenService.isInvalidated(jti)) {
			throw new GlobalException(GlobalException.Type.INVALID_AUTHENTICATION, "Unauthorized");
		}

		return authentication;
	}

	@Transactional
	public UserResponseDto getCurrentUser(Authentication authentication) {
		Long userId = jwtService.extractUserIdFromAuthentication(authentication);
		User user = userService.findByUserIdOrThrow(userId);
		return userMapper.toUserResponseDto(user);
	}

	@Transactional
	public SignInResponseDto refresh(String rawRefreshToken) {
		String jti = jwtService.extractJtiFromRefreshToken(rawRefreshToken);

		refreshTokenService.throwIfIsRevoked(jti);
		refreshTokenService.throwIfIsExpired(jti);

		refreshTokenService.revokeByJti(jti);

		Long userId = jwtService.extractUserIdFromRefreshToken(rawRefreshToken);
		String username = jwtService.extractUsernameFromRefreshToken(rawRefreshToken);

		String newAccessToken = jwtService.generateAccessToken(userId, username);
		String newRefreshToken = jwtService.generateRefreshToken(userId, username);

		RefreshToken refreshToken = RefreshToken.builder()
				.jti(jwtService.extractJtiFromRefreshToken(newRefreshToken))
				.expiryDate(jwtService.extractExpiryFromRefreshToken(newRefreshToken))
				.build();

		refreshTokenService.save(refreshToken);

		return SignInResponseDto.builder()
				.accessToken(newAccessToken)
				.refreshToken(newRefreshToken)
				.build();
	}

	/*
	noinspection
	@Deprecated
	public SignOutResponseDto signOut(SignOutRequestDto request) {
		try {
			SignedJWT signedJWT = verifyToken(request.getAccessToken());

			String jti = signedJWT.getJWTClaimsSet().getJWTID();
			Date expiryDate = signedJWT.getJWTClaimsSet().getExpirationTime();

			InvalidatedAccessToken newInvalidatedToken = InvalidatedAccessToken.builder()
					.jti(jti)
					.expiryDate(expiryDate.toInstant())
					.build();
			InvalidatedAccessToken responseInvalidatedToken = invalidatedTokenService.save(newInvalidatedToken);

			return SignOutResponseDto.builder()
					.invalidatedToken(request.getAccessToken())
					.build();
		} catch (Exception e) {
			throw new GlobalException(GlobalException.Type.INTERNAL_SERVER_ERROR, "Internal server error");
		}
	}
	*/

	/*
	noinspection
	@Deprecated
	private SignedJWT verifyToken(String token) throws JOSEException, ParseException {
		JWSVerifier verifier = new MACVerifier(JwtConstant.SIGNER_KEY.getBytes());

		SignedJWT signedJWT = SignedJWT.parse(token);

		Date expiryTime = signedJWT.getJWTClaimsSet().getExpirationTime();

		var verified = signedJWT.verify(verifier);

		if (!(verified && expiryTime.after(new Date()))) {
			//Exception throw
			return null;
		}

		return signedJWT;
	}
	*/

}