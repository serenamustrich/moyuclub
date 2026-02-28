package com.pacman.controller;

import com.pacman.entity.Leaderboard;
import com.pacman.entity.User;
import com.pacman.service.LeaderboardService;
import com.pacman.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.stream.Collectors;
import java.util.HashMap;

@RestController
@RequestMapping("/api/scores")
public class LeaderboardController {
    @Autowired
    private LeaderboardService leaderboardService;
    @Autowired
    private UserService userService;
    
    // 提交分数
    @PostMapping
    public ResponseEntity<?> submitScore(@RequestBody Map<String, Object> request) {
        try {
            Long userId = ((Number) request.get("user_id")).longValue();
            Integer score = (Integer) request.get("score");
            Integer timeUsed = (Integer) request.get("time_used");
            String deviceType = (String) request.get("device_type");
            
            if (userId == null || score == null || timeUsed == null || deviceType == null) {
                Map<String, Object> errorMap = new HashMap<>();
                errorMap.put("error", "缺少必要参数");
                return ResponseEntity.badRequest().body(errorMap);
            }
            
            User user = userService.findById(userId)
                    .orElseThrow(() -> new RuntimeException("用户不存在"));
            
            Leaderboard leaderboard = leaderboardService.submitScore(user, score, timeUsed, deviceType);
            int rank = leaderboardService.getRank(timeUsed, score);
            
            Map<String, Object> successMap = new HashMap<>();
            successMap.put("message", "分数提交成功");
            successMap.put("rank", rank);
            return ResponseEntity.ok(successMap);
        } catch (Exception e) {
            Map<String, Object> errorMap = new HashMap<>();
            errorMap.put("error", e.getMessage());
            return ResponseEntity.badRequest().body(errorMap);
        }
    }
    
    // 获取排行榜
    @GetMapping
    public ResponseEntity<?> getScores(@RequestParam(defaultValue = "10") int limit) {
        try {
            List<Leaderboard> leaderboardList = leaderboardService.getLeaderboard(limit);
            
            List<Map<String, Object>> scores = leaderboardList.stream().map(leaderboard -> {
                Map<String, Object> scoreMap = new HashMap<>();
                scoreMap.put("rank", leaderboardList.indexOf(leaderboard) + 1);
                scoreMap.put("username", leaderboard.getUser().getUsername());
                scoreMap.put("score", leaderboard.getScore());
                scoreMap.put("time_used", leaderboard.getTimeUsed());
                scoreMap.put("device_type", leaderboard.getDeviceType());
                scoreMap.put("created_at", leaderboard.getCreatedAt().toString());
                return scoreMap;
            }).collect(Collectors.toList());
            
            Map<String, Object> responseMap = new HashMap<>();
            responseMap.put("scores", scores);
            return ResponseEntity.ok(responseMap);
        } catch (Exception e) {
            Map<String, Object> errorMap = new HashMap<>();
            errorMap.put("error", e.getMessage());
            return ResponseEntity.badRequest().body(errorMap);
        }
    }
    
    // 获取用户排名
    @GetMapping("/user/{userId}")
    public ResponseEntity<?> getUserRank(@PathVariable Long userId) {
        try {
            Optional<Leaderboard> userBestScore = leaderboardService.getUserBestScore(userId);
            if (userBestScore.isPresent()) {
                Leaderboard leaderboard = userBestScore.get();
                int rank = leaderboardService.getRank(leaderboard.getTimeUsed(), leaderboard.getScore());
                
                Map<String, Object> rankMap = new HashMap<>();
                rankMap.put("rank", rank);
                rankMap.put("score", leaderboard.getScore());
                rankMap.put("time_used", leaderboard.getTimeUsed());
                return ResponseEntity.ok(rankMap);
            } else {
                Map<String, Object> emptyMap = new HashMap<>();
                emptyMap.put("rank", null);
                return ResponseEntity.ok(emptyMap);
            }
        } catch (Exception e) {
            Map<String, Object> errorMap = new HashMap<>();
            errorMap.put("error", e.getMessage());
            return ResponseEntity.badRequest().body(errorMap);
        }
    }
}
