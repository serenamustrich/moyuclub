package com.pacman.controller;

import com.pacman.entity.User;
import com.pacman.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.Map;
import java.util.HashMap;

@RestController
@RequestMapping("/api/auth")
public class AuthController {
    @Autowired
    private UserService userService;
    
    // 注册
    @PostMapping("/register")
    public ResponseEntity<?> register(@RequestBody Map<String, String> request) {
        try {
            String username = request.get("username");
            String account = request.get("account");
            String password = request.get("password");
            String securityQuestion = request.get("security_question");
            String securityAnswer = request.get("security_answer");
            
            if (username == null || account == null || password == null || securityQuestion == null || securityAnswer == null) {
                Map<String, Object> errorMap = new HashMap<>();
                errorMap.put("error", "缺少必要参数");
                return ResponseEntity.badRequest().body(errorMap);
            }
            
            userService.register(username, account, password, securityQuestion, securityAnswer);
            Map<String, Object> successMap = new HashMap<>();
            successMap.put("message", "注册成功");
            return ResponseEntity.ok(successMap);
        } catch (Exception e) {
            Map<String, Object> errorMap = new HashMap<>();
            errorMap.put("error", e.getMessage());
            return ResponseEntity.badRequest().body(errorMap);
        }
    }
    
    // 登录
    @PostMapping("/login")
    public ResponseEntity<?> login(@RequestBody Map<String, String> request) {
        try {
            String account = request.get("account");
            String password = request.get("password");
            
            if (account == null || password == null) {
                Map<String, Object> errorMap = new HashMap<>();
                errorMap.put("error", "缺少账号或密码");
                return ResponseEntity.badRequest().body(errorMap);
            }
            
            User user = userService.login(account, password)
                    .orElseThrow(() -> new RuntimeException("账号或密码错误"));
            
            Map<String, Object> userMap = new HashMap<>();
            userMap.put("id", user.getId());
            userMap.put("username", user.getUsername());
            
            Map<String, Object> successMap = new HashMap<>();
            successMap.put("message", "登录成功");
            successMap.put("user", userMap);
            return ResponseEntity.ok(successMap);
        } catch (Exception e) {
            Map<String, Object> errorMap = new HashMap<>();
            errorMap.put("error", e.getMessage());
            return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(errorMap);
        }
    }
    
    // 重置密码
    @PostMapping("/reset_password")
    public ResponseEntity<?> resetPassword(@RequestBody Map<String, String> request) {
        try {
            String account = request.get("account");
            String securityAnswer = request.get("security_answer");
            String newPassword = request.get("new_password");
            
            if (account == null || securityAnswer == null || newPassword == null) {
                Map<String, Object> errorMap = new HashMap<>();
                errorMap.put("error", "缺少必要参数");
                return ResponseEntity.badRequest().body(errorMap);
            }
            
            userService.resetPassword(account, securityAnswer, newPassword);
            Map<String, Object> successMap = new HashMap<>();
            successMap.put("message", "密码重置成功");
            return ResponseEntity.ok(successMap);
        } catch (Exception e) {
            Map<String, Object> errorMap = new HashMap<>();
            errorMap.put("error", e.getMessage());
            return ResponseEntity.badRequest().body(errorMap);
        }
    }
    
    // 修改用户名
    @PostMapping("/update_username")
    public ResponseEntity<?> updateUsername(@RequestBody Map<String, Object> request) {
        try {
            Long userId = Long.parseLong(request.get("user_id").toString());
            String newUsername = request.get("new_username").toString();
            
            if (userId == null || newUsername == null) {
                Map<String, Object> errorMap = new HashMap<>();
                errorMap.put("error", "缺少必要参数");
                return ResponseEntity.badRequest().body(errorMap);
            }
            
            userService.updateUsername(userId, newUsername);
            Map<String, Object> successMap = new HashMap<>();
            successMap.put("message", "用户名修改成功");
            return ResponseEntity.ok(successMap);
        } catch (Exception e) {
            Map<String, Object> errorMap = new HashMap<>();
            errorMap.put("error", e.getMessage());
            return ResponseEntity.badRequest().body(errorMap);
        }
    }
}
