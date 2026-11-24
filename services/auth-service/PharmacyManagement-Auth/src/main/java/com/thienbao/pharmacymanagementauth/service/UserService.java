package com.thienbao.pharmacymanagementauth.service;

import com.thienbao.pharmacymanagementauth.dto.request.SignUpRequestDto;
import com.thienbao.pharmacymanagementauth.entity.User;
import com.thienbao.pharmacymanagementauth.exception.entity.GlobalException;
import com.thienbao.pharmacymanagementauth.mapper.UserMapper;
import com.thienbao.pharmacymanagementauth.repository.UserRepository;
import lombok.AccessLevel;
import lombok.RequiredArgsConstructor;
import lombok.experimental.FieldDefaults;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.Optional;

@FieldDefaults(level = AccessLevel.PRIVATE, makeFinal = true)
@RequiredArgsConstructor
@Service
public class UserService {

	/**
	 * Repository
	 */

	UserRepository userRepository;

	/**
	 * Mapper
	 */

	UserMapper userMapper;

	/**
	 * Bean
	 */

	PasswordEncoder passwordEncoder;

	@Transactional
	public User createUser(SignUpRequestDto request) {
		throwIfExistsByUsername(request.getUsername());

		User user = userMapper.toUser(request);
		user.setPasswordHash(passwordEncoder.encode(request.getPassword()));

		return save(user);
	}

	@Transactional
	public void updatePassword(String username, String newPassword) {
		User user = findByUsernameOrThrow(username);

		user.setPasswordHash(passwordEncoder.encode(newPassword));

		save(user);
	}

	@Transactional
	public User save(User user) {
		return userRepository.save(user);
	}

	@Transactional
	public void delete(User user) {
		userRepository.delete(user);
	}

	@Transactional(readOnly = true)
	public User findByUserIdOrThrow(Long userId) {
		return userRepository
				.findById(userId)
				.orElseThrow(() ->
						new GlobalException(GlobalException.Type.NOT_FOUND, "User not found")
				);
	}

	@Transactional
	public User findByUsernameOrThrow(String username) {
		return userRepository
				.findByUsername(username)
				.orElseThrow(() ->
						new GlobalException(GlobalException.Type.NOT_FOUND, "User not found")
				);
	}

	public boolean existsById(Long id) {
		return userRepository.existsById(id);
	}

	@Transactional
	public boolean existsByUsername(String username) {
		return userRepository.existsByUsername(username);
	}

	@Transactional
	public void throwIfExistsByUsername(String username) {
		if (existsByUsername(username)) {
			throw new GlobalException(GlobalException.Type.ALREADY_EXISTS, "User exists");
		}
	}

	@Transactional
	public void throwIfNotExistsByUsername(String username) {
		if (!existsByUsername(username)) {
			throw new GlobalException(GlobalException.Type.NOT_FOUND, "User not found");
		}
	}

	public void throwIfExistsById(Long id) {
		if (existsById(id)) {
			throw new GlobalException(GlobalException.Type.ALREADY_EXISTS, "User exists");
		}
	}

	public void throwIfNotExistsById(Long id) {
		if (!existsById(id)) {
			throw new GlobalException(GlobalException.Type.NOT_FOUND, "User not found");
		}
	}

}