#include "lock_tracer.hpp"
#include "lock_thread_tracer.hpp"

#include <sstream>

learn::lock_tracer learn::lock_tracer::inst;

namespace learn
{

void lock_tracer::add(lock_thread_tracer* tracer)
{
  tracers_[tracer->get_thread_id()] = tracer;
}

std::string lock_tracer::to_string(std::thread::id id) const
{
  auto iter = tracers_.find(id);

  if (iter == tracers_.end())
  {
    return "<>";
  }

  return iter->second->to_string();
}

std::string lock_tracer::to_string() const
{
  std::ostringstream oss;

  for (auto& kv : tracers_)
  {
    oss << "thread: " << kv.second->get_thread_id();
    oss << kv.second->to_string();
    oss << std::endl;
  }

  return oss.str();
}

} // learn