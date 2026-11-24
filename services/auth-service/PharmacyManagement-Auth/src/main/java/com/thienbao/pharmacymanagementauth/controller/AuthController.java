package com.thienbao.pharmacymanagementauth.controller;

import com.thienbao.pharmacymanagementauth.dto.ResponseDTO;
import com.thienbao.pharmacymanagementauth.dto.request.SignUpRequestDto;
import com.thienbao.pharmacymanagementauth.dto.request.SignUpResponseDto;
import com.thienbao.pharmacymanagementauth.dto.response.UserResponseDto;
import com.thienbao.pharmacymanagementauth.dto.signin.SignInRequestDto;
import com.thienbao.pharmacymanagementauth.dto.signin.SignInResponseDto;
import com.thienbao.pharmacymanagementauth.dto.signout.SignOutResponseDto;
import com.thienbao.pharmacymanagementauth.service.AuthService;
import lombok.AccessLevel;
import lombok.RequiredArgsConstructor;
import lombok.experimental.FieldDefaults;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PatchMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestHeader;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@FieldDefaults(level = AccessLevel.PRIVATE, makeFinal = true)
@RequiredArgsConstructor
@RequestMapping("/api/auth")
@RestController
public class AuthController {

	AuthService authService;

	@PostMapping("/signin")
	public ResponseDTO<SignInResponseDto> signIn(
			@RequestBody SignInRequestDto request
	) {
		SignInResponseDto signInResponseDto = authService.signInInternal(request);

		return ResponseDTO.<SignInResponseDto>builder()
				.success(true)
				.message("Sign in successful")
				.data(signInResponseDto)
				.build();
	}

	@PostMapping("/signup")
	public ResponseDTO<SignUpResponseDto> signUp(
			@RequestBody SignUpRequestDto request
	) {
		SignUpResponseDto signUpResponseDto = authService.signUpInternal(request);

		return ResponseDTO.<SignUpResponseDto>builder()
				.success(true)
				.message("Sign up successful")
				.data(signUpResponseDto)
				.build();
	}

	@PreAuthorize("@TokenSecurity.isValid()")
	@PostMapping("/signout")
	public ResponseDTO<SignOutResponseDto> signOut(
			@RequestHeader(value = "X-Refresh-Token", required = false) String refreshToken
	) {
		System.out.println(refreshToken);
		Authentication authentication = authService.getAuthentication();
		SignOutResponseDto signOutResponseDto = authService.signOut(authentication, refreshToken);

		return ResponseDTO.<SignOutResponseDto>builder()
				.success(true)
				.message("Sign out successful")
				.data(signOutResponseDto)
				.build();
	}

	@PostMapping("/refresh")
	public ResponseDTO<SignInResponseDto> refresh(
			@RequestHeader(name = "X-Refresh-Token") String refreshToken
	) {
		SignInResponseDto response = authService.refresh(refreshToken);

		return ResponseDTO.<SignInResponseDto>builder()
				.success(true)
				.message("Refresh tokens successful")
				.data(response)
				.build();
	}

	@GetMapping("/validate")
	public ResponseDTO<Void> validate() {
		return ResponseDTO.ok("Validated");
	}

}