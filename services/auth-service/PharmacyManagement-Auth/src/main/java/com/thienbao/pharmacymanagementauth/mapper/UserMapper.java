package com.thienbao.pharmacymanagementauth.mapper;

import com.thienbao.pharmacymanagementauth.config.MapStructConfig;
import com.thienbao.pharmacymanagementauth.dto.request.SignUpRequestDto;
import com.thienbao.pharmacymanagementauth.dto.response.UserResponseDto;
import com.thienbao.pharmacymanagementauth.entity.User;
import org.mapstruct.BeanMapping;
import org.mapstruct.Mapper;
import org.mapstruct.MappingTarget;
import org.mapstruct.NullValuePropertyMappingStrategy;

@Mapper(config = MapStructConfig.class)
public interface UserMapper {
	User toUser(SignUpRequestDto request);
	UserResponseDto toUserResponseDto(User user);
}